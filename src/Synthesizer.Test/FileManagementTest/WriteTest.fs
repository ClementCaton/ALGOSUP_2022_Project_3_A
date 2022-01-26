module Synthesizer.writeTest

open NUnit.Framework
open System
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Write () =
    let writer = new writeWav()
    use stream = new MemoryStream()
    writer.Write stream [API.createSound 440. Quarter 1. Sin]
    
    Assert.IsTrue(File.Exists("./Output/wave.wav"))
