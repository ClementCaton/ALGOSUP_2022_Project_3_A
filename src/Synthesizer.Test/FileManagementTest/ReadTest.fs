module Synthesizer.readTest

open System
open NUnit.Framework
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ReadTest() =
    Synth().WriteToWav "wav.wav" [Synth().Sound 440. Quarter Sin]
    use stream = File.Open("Output/wav.wav", FileMode.Open)
    let theFile = ReadWav().Read (stream)
    
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())