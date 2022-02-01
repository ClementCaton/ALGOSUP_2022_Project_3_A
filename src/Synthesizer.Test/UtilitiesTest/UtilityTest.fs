module Synthesizer.utilityTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

let One = Seconds 1.

[<Test>]
let CutStartTest() =
    let Data = Synth.Note One Note.C 4 |> Utility.CutStart 44100. 0.2

    Assert.That(Data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(Data |> List.max, Is.LessThan(1))
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let CutEndTest() =
    let Data = Synth.Note One Note.C 4 |> Utility.CutEnd 44100. 0.2

    Assert.That(Data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(Data |> List.max, Is.LessThan(1))
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let CutCornersTest() =
    let Data = Synth.Note One Note.C 4 |> Utility.CutCorners 800 |> Utility.CutEnd 44100. 0.98
    let MockData = Synth.Note One Note.C 4 |> Utility.CutEnd 44100. 0.98

    Assert.That (Data, Is.LessThan MockData)
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let addTest() =
    let Data = Synth.Note One Note.C 4
    let Data2 = Synth.Note One Note.C 7

    let FinalData = Utility.Add [Data; Data2]

    Assert.That(Data.Length, Is.EqualTo(44100))
    Assert.That(FinalData |> List.max, Is.LessThan(1))
    Assert.That (FinalData, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let OverdriveTest() =
    let Duration = 90. // In seconds
    let SampleRate = 44100.
    let ArraySize = int ((44100.) * Duration)
    let Amplitude = 1.
    let PhaseShift = 0.
    let VerticalShift = 0.
    let Frequency = 440.
    let OverDrive = 1.

    let A = List.init ArraySize (fun i -> ((FourWaves.SinWave) Frequency Amplitude VerticalShift PhaseShift (float i/SampleRate)))
    let Data = Utility.Overdrive 1 a

    Assert.That(Data.Length, Is.EqualTo(ArraySize))
    Assert.That(Data |> List.max, Is.LessThan(1))
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))