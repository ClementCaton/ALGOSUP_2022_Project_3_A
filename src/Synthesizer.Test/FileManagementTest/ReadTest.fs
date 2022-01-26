module Synthesizer.readTest

open System
open NUnit.Framework
open System.IO

[<SetUp>]
let Setup () =
    ()

[<Test>]
let readFromWavApiTest() =
    API.writeToWav "./wav.wav" [API.createSound 440. Quarter 1. Sin]
    use stream = File.Open("./Output/wav.wav", FileMode.Open)
    let theFile = readWav().Read (stream)
    
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())