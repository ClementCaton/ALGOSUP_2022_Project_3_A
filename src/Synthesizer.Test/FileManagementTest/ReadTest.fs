module Synthesizer.readTest

open System
open NUnit.Framework
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ReadTest() =
    Synth.WriteToWav "wav.wav" [Synth.Sound 440. Quarter Sin]
    use Stream = File.Open("Output/wav.wav", FileMode.Open)
    let TheFile = ReadWav().Read (Stream)
    
    Assert.That(TheFile, Is.InstanceOf<List<List<float>> * float * int * int>())