module Synthesizer.writeTest

open NUnit.Framework
open System
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let writeTest () =
    let writer = new writeWav()
    use stream = new MemoryStream()
    writer.Write stream [Synth.Sound 440. Quarter Sin]
    
    Assert.IsTrue(File.Exists("./Output/wave.wav"))
