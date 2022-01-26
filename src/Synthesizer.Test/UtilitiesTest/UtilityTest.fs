module Synthesizer.utilityTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

let one = Seconds 1.

[<Test>]
let cutStartUtilityTest() =
    let data = Synth.note one Note.C 4 |> Utility.cutStart 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let cutEndUtilityTest() =
    let data = Synth.note one Note.C 4 |> Utility.cutEnd 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let cutCornersUtilityTest() =
    let data = Synth.note one Note.C 4 |> Utility.cutCorners 800 |> Utility.cutEnd 44100. 0.98
    let mockData = Synth.note one Note.C 4 |> Utility.cutEnd 44100. 0.98

    Assert.That (data, Is.LessThan mockData)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let addUtilityTest() =
    let data = Synth.note one Note.C 4
    let data2 = Synth.note one Note.C 7

    let finalData = Utility.add [data; data2]

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

    let a = List.init arraySize (fun i -> ((fourWaves.sinWave) frequency amplitude verticalShift phaseShift (float i/sampleRate)))
    let data = Utility.Overdrive 1 a

    Assert.That(data.Length, Is.EqualTo(arraySize))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))