module GitHubReleaseUpdate 

open Functional.SplinterBots.API
open System.IO
open System.Runtime.InteropServices
open System.Net
open System.Net.Http
open System.Reflection

let private apiUrl = "https://api.github.com/repos/functional-solutions/Splinterbots/releases"
let notSupported = "NOT_SUPPORTED"

type AssetDownloadInfo = 
    {
        name: string
        browser_download_url: string
    }
type GitHubRequest = 
    {
        name: string
        assets: AssetDownloadInfo array
    }

let getLatestGithubVersion () =
    async {
        let! githubReleases = API.executeApiCall<GitHubRequest list> apiUrl
        match githubReleases with 
        | [] -> 
            return None
        | head :: tail -> 
            return Some head
    }

let compareWithApplicationVersion releaseInfo =
    async {
        let applicationVersion = Version.getVersion ()
        let newestVersion = releaseInfo.name |> Version.bind

        let shouldUpdate = Version.isFirstVersionLarger newestVersion applicationVersion  

        match shouldUpdate with
        | true -> 
            printfn $"Application version: {applicationVersion}"
            printfn $"Server version: {newestVersion}"
            return Some releaseInfo
        | _ -> 
            return None
    }

let private getAssetsUrl (assetNamePrefix: string) releaseInfo = 
    let asset =  
        releaseInfo.assets
        |> Array.find (fun asset -> asset.name.StartsWith(assetNamePrefix))
    asset.browser_download_url

let getProperAssetUrl ghReleaseInfo = 
    async {
        match RuntimeInformation.OSArchitecture with 
        | Architecture.X64 ->
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            then 
                return Some (getAssetsUrl "linux-x64-" ghReleaseInfo)

            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            then 
                return Some (getAssetsUrl "win-x64" ghReleaseInfo)

            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            then 
                return Some (getAssetsUrl "osx-x64" ghReleaseInfo)

            else 
                return None
        | Architecture.Arm64 ->
            return Some (getAssetsUrl "linux-arm64-" ghReleaseInfo)
        | _ ->
            return None
    }

let downloadZip (downloadUrl: string) = 
    async {
        use client = new HttpClient()
        let! response = client.GetByteArrayAsync downloadUrl |> Async.AwaitTask

        let tempFileLocation = Path.Combine(Path.GetTempPath (), Path.GetRandomFileName () + ".zip")
        File.WriteAllBytes(tempFileLocation, response)
        return Some tempFileLocation
    }

let unzip sourceFile =
    async {
        let destinationPath = Path.Combine(Path.GetTempPath (), Path.GetRandomFileName())
        System.IO.Compression.ZipFile.ExtractToDirectory(sourceFile, destinationPath)
        return Some destinationPath
    }

let deployNewApplicationToTemporaryFolder applicationLocation tempFolderLocation  = 
    async {
        let currentFolder = 
            let newPath = Path.Combine (applicationLocation, "new")
            Directory.CreateDirectory newPath |> ignore
            newPath
        let moveFiles (sourceFile: string) = 
            let fileName =  Path.GetFileName sourceFile
            let destinationFile = Path.Combine (currentFolder, fileName)
            File.Move (sourceFile, destinationFile, true)
        Directory.GetFiles tempFolderLocation
        |> Array.iter moveFiles

        return Some ()
    }

let updateApplication applicationLocation =
    let (|>>) input action =
        async {
            let! input = input
            match input with 
            | Some x -> 
                let! result = action x
                return result
            | None -> 
                return None 
        }
    async {
        let computation =
            getLatestGithubVersion ()
            |>> compareWithApplicationVersion
            |>> getProperAssetUrl
            |>> downloadZip
            |>> unzip
            |>> deployNewApplicationToTemporaryFolder applicationLocation

        let! result = async.TryWith(computation, fun exn -> async.Return None)

        return result.IsSome
    }
