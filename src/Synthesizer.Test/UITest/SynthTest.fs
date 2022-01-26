module Synthesizer.SynthTest

open System
open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let getNoteFreqTest() =
    let val1 = Math.Round( Synth.getNoteFreq Note.A 4, 2)
    let val2 = Math.Round( Synth.getNoteFreq Note.B 5, 2)

    Assert.That(val1, Is.EqualTo 440.)
    Assert.That(val2, Is.EqualTo 987.77)

[<Test>]
let getNoteFreqWithOffsetSynthTest() =
    let val1 = Math.Round( Synth.getNoteFreqOffset Note.A 4 436, 2)
    let val2 = Math.Round( Synth.getNoteFreqOffset Note.B 5 444, 2)

    Assert.That(val1, Is.EqualTo 436.)
    Assert.That(val2, Is.EqualTo 996.75)


let one = Seconds 1.

[<Test>]
let SoundSynthTest() =
    let data = Synth.Sound 440. one Sin

    Assert.That(data, Is.TypeOf<List<float>>()) 
    Assert.That(data |> List.max, Is.LessThan 1)
    Assert.That(data.Length, Is.EqualTo 44100)
    // default sample rate

open System.IO
[<Test>]
let writeToWavSynthTest() =
    Synth.writeToWav "wave.wav" [Synth.Sound 440. one Sin]
    Assert.IsTrue(File.Exists("./wave.wav"))

[<Test>]
let readFromWavSynthTest() =
    let theFile = Synth.readFromWav "wave.wav"
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())

[<Test>]
let noteSynthTest() =
    let data = Synth.note Quarter Note.C 4

    
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let silenceSynthTest() =
    let data = Synth.silence one

    Assert.That (data |> List.max, Is.EqualTo 0)
    Assert.That (data.Length, Is.EqualTo 44100)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let composeSynthTest() =
    let data = Synth.compose [
        Synth.note one Note.C 4
        Synth.note one Note.C 4
    ]

    Assert.That(data.Length, Is.EqualTo(44100*2))
    Assert.That (data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let addSynthTest() =
    let data = Synth.note one Note.C 4
    let data2 = Synth.note one Note.C 7

    let finalData = Synth.add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))