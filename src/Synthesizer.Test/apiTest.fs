module Synthesizer.apiTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let getNoteFreqTest() =
    let val1 = API.getNoteFreq Note.A 4
    let val2 = API.getNoteFreq Note.B 5

    Assert.That(val1, Is.EqualTo 440.)
    Assert.That(val2, Is.EqualTo 987.77)

[<Test>]
let getNoteFreqWithOffsetApiTest() =
    let val1 = API.getNoteFreqOffset Note.A 4 436
    let val2 = API.getNoteFreqOffset Note.B 5 444

    Assert.That(val1, Is.EqualTo 436.)
    Assert.That(val2, Is.EqualTo 996.75)


let one = Seconds 1.

[<Test>]
let createSoundApiTest() =
    let data = API.createSound 440. one Sin

    Assert.That(data, Is.TypeOf<List<float>>()) 
    Assert.That(data |> List.max, Is.LessThan 1)
    Assert.That(data.Length, Is.EqualTo 44100)
    // default sample rate

open System.IO
[<Test>]
let writeToWavApiTest() =
    API.writeToWav "wave.wav" (API.createSound 440. one Sin)
    Assert.IsTrue(File.Exists("./wave.wav"))

[<Test>]
let readFromWavApiTest() =
    let theFile = API.readFromWav "wave.wav"
    Assert.That(theFile, Is.InstanceOf<List<float> * float * int * int * int>())

[<Test>]
let noteApiTest() =
    let data = API.note Quarter Note.C 4

    
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let silenceApiTest() =
    let data = API.silence one

    Assert.That (data |> List.max, Is.EqualTo 0)
    Assert.That (data.Length, Is.EqualTo 44100)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let composeApiTest() =
    let data = API.compose [
        API.note one Note.C 4
        API.note one Note.C 4
    ]

    Assert.That(data.Length, Is.EqualTo(44100*2))
    Assert.That (data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let addApiTest() =
    let data = API.note one Note.C 4
    let data2 = API.note one Note.C 7

    let finalData = API.add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))