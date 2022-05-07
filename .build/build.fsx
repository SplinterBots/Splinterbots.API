#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "./artifacts"
    |> Shell.cleanDirs 

    Directory.ensure "./artifacts"
)

Target.create "Publish" (fun _ ->
    let files = 
        !! "src/**/*.nupkg"
        |> Array.ofSeq

    files |> Seq.iter (fun file -> Shell.copyFile "./artifacts/" file)
)
Target.create "Build" (fun _ ->
    let packConfiguration (defaults:DotNet.PackOptions) =
        { defaults with
              Configuration = DotNet.Release
              IncludeSymbols = true }

    !! "src/**/*.fsproj"
    |> Seq.iter (DotNet.pack packConfiguration)
)

"Clean"
  ==> "Build"

"Build"
  ==> "Publish"

Target.runOrDefault "Build"
