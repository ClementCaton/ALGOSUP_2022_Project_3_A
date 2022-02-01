module Synthesizer.filtersTest

open System
open NUnit.Framework
open Synthesizer.Filter

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ChangeAmplitudeTest() =
    let Creater1 = new SoundData (Amplitude0 = 0.5)
    let Data = ChangeAmplitude 2. (Creater1.Create Sin)
    let Creater2 = new SoundData (Amplitude0 = 1.)
    let MockData = Creater2.Create Sin

    Assert.That(MockData,Is.EqualTo(Data))
    Assert.That(Data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesTest() =
    let Data = AddTwoWaves 0.5 (SoundData(Amplitude0 = 0.).Create Sin) (SoundData(Amplitude0 = 1.).Create Sin)
    let MockData = (SoundData(Amplitude0 = 0.5).Create Sin)

    Assert.That(MockData,Is.EqualTo(Data))
    Assert.That(Data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesWithASilentTest() =
    let Data = AddTwoWaves 0.5 (SoundData(Amplitude0 = 1.).Create Sin) (SoundData(Amplitude0 = 0.).Create Sin)
    let MockData = (SoundData(Amplitude0 = 0.5).Create Sin)

    Assert.That(MockData,Is.EqualTo(Data))
    Assert.That(Data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesFirstRatioTest() =
    let Data = AddTwoWaves 1. (SoundData(Amplitude0 = 1.).Create Sin) (SoundData(Amplitude0 = 0.5).Create Sin)
    let MockData = SoundData(Amplitude0 = 1.).Create Sin

    Assert.That(MockData,Is.EqualTo(Data))
    Assert.That(Data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesSecondRatioTest() =
    let Data = AddTwoWaves 0 (SoundData(Amplitude0 = 1.).Create Sin) (SoundData(Amplitude0 = 0.5).Create Sin)
    let MockData = SoundData(Amplitude0 = 0.5).Create Sin

    Assert.That(MockData,Is.EqualTo(Data))
    Assert.That(Data,Is.InstanceOf<List<float>>())
    
[<Test>]
let AddModulationWithASilentTest() =
    let Data = AddModulation 0.5 (SoundData(Amplitude0 = 0.).Create Silence) (SoundData(Amplitude0 = 1.).Create Sin)
    let MockData = (SoundData(Amplitude0 = 1.).Create Sin)
    
    let RoundData = Data |> List.map (fun x -> Math.Round(x, 10))
    let RoundMockData = MockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(RoundMockData,Is.EqualTo(RoundData))
    Assert.That(Data,Is.InstanceOf<List<float>>())


[<Test>]
let AddModulationFirstRatioTest() =
    let Data = AddModulation 1. (SoundData(Amplitude0 = 0.5).Create Sin) (SoundData(Amplitude0 = 1.).Create Sin)
    let MockData = SoundData(Amplitude0 = 0.5).Create Sin

    let RoundData = Data |> List.map (fun x -> Math.Round(x, 10))
    let RoundMockData = MockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(RoundMockData.[300],Is.EqualTo(RoundData.[300]))
    Assert.That(RoundData,Is.InstanceOf<List<float>>())

[<Test>]
let AddModulationSecondRatioTest() =
    let Data = addModulation 0. (SoundData(Amplitude0 = 0.).Create Sin) (SoundData(Amplitude0 = 0.5).Create Sin)
    let MockData = SoundData(Amplitude0 = 0.5).Create Sin

    let RoundData = Data |> List.map (fun x -> Math.Round(x, 10))
    let RoundMockData = MockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(RoundMockData.[300],Is.EqualTo(RoundData.[300]))
    Assert.That(Data,Is.InstanceOf<List<float>>())