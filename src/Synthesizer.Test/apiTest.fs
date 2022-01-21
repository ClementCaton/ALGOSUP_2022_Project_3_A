module Synthesizer.apiTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let getNoteApiTest() =
    let val1 = API.getNoteFreq Note.A 4
    let val2 = API.getNoteFreq Note.B 5

    Assert.That(val1, Is.EqualTo(440.))
    Assert.That(val2, Is.EqualTo(987.77))

[<Test>]
let getNoteWithOffsetApiTest() =
    let val1 = API.getNoteFreqOffset Note.A 4 436
    let val2 = API.getNoteFreqOffset Note.B 5 444

    Assert.That(val1, Is.EqualTo(436.))
    Assert.That(val2, Is.EqualTo(996.75))


let one = Seconds 1.
[<Test>]
let createSoundApiTest() =
    let final = API.createSound 440. one Sin

    Assert.That(final, Is.TypeOf<List<float>>()) 
    Assert.That(final |> List.max, Is.LessThan(1))
    Assert.That(final.Length, Is.EqualTo(44100))
    // default sample rate
[<Test>]
let apiSilenceTest() =
    let data = API.silence Quarter

    let mockData = List.init (int(44100.*60./114.)) (fun i -> 0.)

    Assert.That (data, Is.EqualTo mockData)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

open System.IO
[<Test>]
let writeToWavApiTest() =
    API.writeToWav "wave.wav" (API.createSound 440. one Sin)
    Assert.IsTrue(File.Exists("./wave.wav"))

[<Test>]
let readFromWavApiTest() =
    let theFile = API.readFromWav "wave.wav"
    Assert.That(theFile, Is.InstanceOf<List<float> * float * int * int * int>())
