module Synthesizer.SynthTest

open System
open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let GetNoteFreqTest() =
    let val1 = Math.Round( Synth.GetNoteFreq Note.A 4, 2)
    let val2 = Math.Round( Synth.GetNoteFreq Note.B 5, 2)

    Assert.That(val1, Is.EqualTo 440.)
    Assert.That(val2, Is.EqualTo 987.77)

[<Test>]
let GetNoteFreqWithOffsetTest() =
    let val1 = Math.Round( Synth.GetNoteFreqOffset Note.A 4 436, 2)
    let val2 = Math.Round( Synth.GetNoteFreqOffset Note.B 5 444, 2)

    Assert.That(val1, Is.EqualTo 436.)
    Assert.That(val2, Is.EqualTo 996.75)


let one = Seconds 1.

[<Test>]
let SoundTest() =
    let data = Synth.Sound 440. one Sin

    Assert.That(data, Is.TypeOf<List<float>>()) 
    Assert.That(data |> List.max, Is.LessThan 1)
    Assert.That(data.Length, Is.EqualTo 44100)
    // default sample rate

open System.IO
[<Test>]
let WriteToWavTest() =
    Synth.WriteToWav "wave.wav" [Synth.Sound 440. one Sin]
    Assert.IsTrue(File.Exists("./Output/wave.wav"))

[<Test>]
let WriteToWavWithPathTest() =
    Synth.WriteToWavWithPath "./Output" "wave.wav" [Synth.Sound 440. one Sin]
    Assert.IsTrue(File.Exists("./Output/wave.wav"))

[<Test>]
let ReadFromWavTest() =
    let TheFile = Synth.ReadFromWav "wave.wav"
    Assert.That(TheFile, Is.InstanceOf<List<List<float>> * float * int * int>())

    
[<Test>]
let ReadFromWavFromPathTest() =
    let TheFile = Synth.ReadFromWavWithPath "Output/wave.wav"
    Assert.That(TheFile, Is.InstanceOf<List<List<float>> * float * int * int>())

[<Test>]
let NoteTest() =
    let data = Synth.Note Quarter Note.C 4

    
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let SilenceTest() =
    let data = Synth.Silence one

    Assert.That (data |> List.max, Is.EqualTo 0)
    Assert.That (data.Length, Is.EqualTo 44100)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let ComposeTest() =
    let data = Synth.Compose [
        Synth.Note one Note.C 4
        Synth.Note one Note.C 4
    ]

    Assert.That(data.Length, Is.EqualTo(44100*2))
    Assert.That (data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let AddTest() =
    let data = Synth.Note one Note.C 4
    let data2 = Synth.Note one Note.C 7

    let finalData = Synth.Add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))