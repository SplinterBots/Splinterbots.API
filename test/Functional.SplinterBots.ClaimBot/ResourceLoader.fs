module ResourceLoader

open System.IO
open System.Reflection

let embeddedResource (name: string) = 
    let assembly = Assembly.GetExecutingAssembly ()
    let assemblyName = assembly.GetName ()
    let stream = assembly.GetManifestResourceNames ()
    use reader = new StreamReader(assembly.GetManifestResourceStream($"{assemblyName.Name}.{name}"))
    let content = reader.ReadToEnd ()
    content

