module Synthesizer.SynthTest

open System
open NUnit.Framework

let synth = Synth()
let one = Seconds 1.

[<SetUp>]
let Setup () =
    ()

[<Test>]
let GetNoteFreqTest() =
    let val1 = Math.Round( synth.GetNoteFreq Note.A 4, 2)
    let val2 = Math.Round( synth.GetNoteFreq Note.B 5, 2)

    Assert.That(val1, Is.EqualTo 440.)
    Assert.That(val2, Is.EqualTo 987.77)

[<Test>]
let GetNoteFreqWithOffsetTest() =
    let val1 = Math.Round( synth.GetNoteFreqOffset Note.A 4 436, 2)
    let val2 = Math.Round( synth.GetNoteFreqOffset Note.B 5 444, 2)

    Assert.That(val1, Is.EqualTo 436.)
    Assert.That(val2, Is.EqualTo 996.75)

[<Test>]
let SoundTest() =
    let data = synth.Sound 440. one Sin

    Assert.That(data, Is.TypeOf<List<float>>()) 
    Assert.That(data |> List.max, Is.LessThan 1)
    Assert.That(data.Length, Is.EqualTo 44100)
    // default sample rate

open System.IO
[<Test>]
let WriteToWavTest() =
    synth.WriteToWav "wave.wav" [synth.Sound 440. one Sin]
    Assert.IsTrue(File.Exists("./Output/wave.wav"))

[<Test>]
let WriteToWavWithPathTest() =
    synth.WriteToWavWithPath "./Output" "wave.wav" [synth.Sound 440. one Sin]
    Assert.IsTrue(File.Exists("./Output/wave.wav"))

[<Test>]
let ReadFromWavTest() =
    let theFile = synth.ReadFromWav "wave.wav"
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())

    
[<Test>]
let ReadFromWavFromPathTest() =
    let theFile = synth.ReadFromWavWithPath "Output/wave.wav"
    Assert.That(theFile, Is.InstanceOf<List<List<float>> * float * int * int>())

[<Test>]
let NoteTest() =
    let data = synth.Note Quarter Note.C 4

    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let SilenceTest() =
    let data = synth.Silence one

    Assert.That (data |> List.max, Is.EqualTo 0)
    Assert.That (data.Length, Is.EqualTo 44100)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let ComposeTest() =
    let data = synth.Compose [
        synth.Note one Note.C 4
        synth.Note one Note.C 4
    ]

    Assert.That(data.Length, Is.EqualTo(44100*2))
    Assert.That (data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let AddTest() =
    let data = synth.Note one Note.C 4
    let data2 = synth.Note one Note.C 7

    let finalData = synth.Add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))