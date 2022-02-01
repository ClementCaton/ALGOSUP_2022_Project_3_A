module Synthesizer.writeTest

open NUnit.Framework
open System
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let WriteTest () =
    let Writer = new WriteWav()
    use Stream = new MemoryStream()
    Writer.Write Stream [Synth.Sound 440. Quarter Sin]
    
    Assert.IsTrue(File.Exists("./Output/wave.wav"))
