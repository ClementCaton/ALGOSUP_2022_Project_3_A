module Synthesizer.SynthTest

open System
open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let GetNoteFreqTest() =
    let Val1 = Math.Round( Synth.getNoteFreq Note.A 4, 2)
    let Val2 = Math.Round( Synth.getNoteFreq Note.B 5, 2)

    Assert.That(Val1, Is.EqualTo 440.)
    Assert.That(Val2, Is.EqualTo 987.77)

[<Test>]
let GetNoteFreqWithOffsetTest() =
    let Val1 = Math.Round( Synth.GetNoteFreqOffset Note.A 4 436, 2)
    let Val2 = Math.Round( Synth.GetNoteFreqOffset Note.B 5 444, 2)

    Assert.That(Val1, Is.EqualTo 436.)
    Assert.That(Val2, Is.EqualTo 996.75)


let One = Seconds 1.

[<Test>]
let SoundTest() =
    let Data = Synth.Sound 440. One Sin

    Assert.That(Data, Is.typeof<List<float>>()) 
    Assert.That(Data |> List.max, Is.LessThan 1)
    Assert.That(Data.Length, Is.EqualTo 44100)
    // default sample rate

open System.IO
[<Test>]
let WriteToWavTest() =
    Synth.WriteToWav "wave.wav" [Synth.Sound 440. One Sin]
    Assert.IsTrue(File.Exists("./Output/wave.wav"))

[<Test>]
let WriteToWavWithPathTest() =
    Synth.WriteToWavWithPath "./Output" "wave.wav" [Synth.Sound 440. One Sin]
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
    let Data = Synth.note Quarter Note.C 4

    
    Assert.That(Data |> List.max, Is.LessThan(1))
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let SilenceTest() =
    let Data = Synth.Silence One

    Assert.That (Data |> List.max, Is.EqualTo 0)
    Assert.That (Data.Length, Is.EqualTo 44100)
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let ComposeTest() =
    let Data = Synth.Compose [
        Synth.Note One Note.C 4
        Synth.Note One Note.C 4
    ]

    Assert.That(Data.Length, Is.EqualTo(44100*2))
    Assert.That (Data |> List.max, Is.LessThan(1))
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let AddTest() =
    let Data = Synth.note One Note.C 4
    let Data2 = Synth.note One Note.C 7

    let FinalData = Synth.Add [Data; Data2]

    Assert.That(Data.Length, Is.EqualTo(44100))
    Assert.That(FinalData |> List.max, Is.LessThan(1))
    Assert.That (FinalData, Is.InstanceOf(typeof<List<float>>))