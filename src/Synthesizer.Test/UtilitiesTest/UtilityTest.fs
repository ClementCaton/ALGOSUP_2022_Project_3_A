module Synthesizer.utilityTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

let synth = Synth()
let one = Seconds 1.

[<Test>]
let CutStartTest() =
    let data = synth.Note one Note.C 4 |> Utility.CutStart 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let CutEndTest() =
    let data = synth.Note one Note.C 4 |> Utility.CutEnd 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let CutCornersTest() =
    let data = synth.Note one Note.C 4 |> Utility.CutCorners 800 |> Utility.CutEnd 44100. 0.98
    let mockData = synth.Note one Note.C 4 |> Utility.CutEnd 44100. 0.98

    Assert.That (data, Is.LessThan mockData)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let AddTest() =
    let data = synth.Note one Note.C 4
    let data2 = synth.Note one Note.C 7

    let finalData = Utility.Add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let OverdriveTest() =
    let duration = 90. // In seconds
    let sampleRate = 44100.
    let arraySize = int ((44100.) * duration)
    let amplitude = 1.
    let phaseShift = 0.
    let verticalShift = 0.
    let frequency = 440.
    let overDrive = 1.

    let a = List.init arraySize (fun i -> ((FourWaves.SinWave) frequency amplitude verticalShift phaseShift (float i/sampleRate)))
    let data = Utility.Overdrive 1 a

    Assert.That(data.Length, Is.EqualTo(arraySize))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))