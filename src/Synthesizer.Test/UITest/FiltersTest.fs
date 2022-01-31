module Synthesizer.filtersTest

open System
open NUnit.Framework
open Synthesizer.Filter

[<SetUp>]
let Setup () =
    ()

[<Test>]
let changeAmplitudeTest() =
    let creater1 = new SoundData (amplitude0 = 0.5)
    let data = changeAmplitude 2. (creater1.create Sin)
    let creater2 = new SoundData (amplitude0 = 1.)
    let mockData = creater2.create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let addTwoWavesTest() =
    let data = addTwoWaves 0.5 (SoundData(amplitude0 = 0.).create Sin) (SoundData(amplitude0 = 1.).create Sin)
    let mockData = (SoundData(amplitude0 = 0.5).create Sin)

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let addTwoWavesWithASilentTest() =
    let data = addTwoWaves 0.5 (SoundData(amplitude0 = 1.).create Sin) (SoundData(amplitude0 = 0.).create Sin)
    let mockData = (SoundData(amplitude0 = 0.5).create Sin)

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let addTwoWavesFirstRatioTest() =
    let data = addTwoWaves 1. (SoundData(amplitude0 = 1.).create Sin) (SoundData(amplitude0 = 0.5).create Sin)
    let mockData = SoundData(amplitude0 = 1.).create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let addTwoWavesSecondRatioTest() =
    let data = addTwoWaves 0 (SoundData(amplitude0 = 1.).create Sin) (SoundData(amplitude0 = 0.5).create Sin)
    let mockData = SoundData(amplitude0 = 0.5).create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())
    
[<Test>]
let addModulationWithASilentTest() =
    let data = addModulation 0.5 (SoundData(amplitude0 = 0.).create Silence) (SoundData(amplitude0 = 1.).create Sin)
    let mockData = (SoundData(amplitude0 = 1.).create Sin)
    
    let roundData = data |> List.map (fun x -> Math.Round(x, 10))
    let roundMockData = mockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(roundMockData,Is.EqualTo(roundData))
    Assert.That(data,Is.InstanceOf<List<float>>())


[<Test>]
let addModulationFirstRatioTest() =
    let data = addModulation 1. (SoundData(amplitude0 = 0.5).create Sin) (SoundData(amplitude0 = 1.).create Sin)
    let mockData = SoundData(amplitude0 = 0.5).create Sin

    let roundData = data |> List.map (fun x -> Math.Round(x, 10))
    let roundMockData = mockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(roundMockData.[300],Is.EqualTo(roundData.[300]))
    Assert.That(roundData,Is.InstanceOf<List<float>>())

[<Test>]
let addModulationSecondRatioTest() =
    let data = addModulation 0. (SoundData(amplitude0 = 0.).create Sin) (SoundData(amplitude0 = 0.5).create Sin)
    let mockData = SoundData(amplitude0 = 0.5).create Sin

    let roundData = data |> List.map (fun x -> Math.Round(x, 10))
    let roundMockData = mockData |> List.map (fun x -> Math.Round(x, 10))

    Assert.That(roundMockData.[300],Is.EqualTo(roundData.[300]))
    Assert.That(data,Is.InstanceOf<List<float>>())