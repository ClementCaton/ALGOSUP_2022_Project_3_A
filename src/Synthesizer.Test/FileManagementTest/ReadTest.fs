module Synthesizer.readTest

open System
open NUnit.Framework
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let readTest() =
    Synth.writeToWav "wav.wav" [Synth.Sound 440. Quarter Sin]
    use stream = File.Open("Output/wav.wav", FileMode.Open)
    let theFile = readWav().Read (stream)
    
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())