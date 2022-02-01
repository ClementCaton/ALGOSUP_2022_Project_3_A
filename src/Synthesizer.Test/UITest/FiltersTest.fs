module Synthesizer.filtersTest

open System
open NUnit.Framework
open Synthesizer.Filter

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ChangeAmplitudeTest() =
    let creater1 = new SoundData (amplitude0 = 0.5)
    let data = ChangeAmplitude 2. (creater1.Create Sin)
    let creater2 = new SoundData (amplitude0 = 1.)
    let mockData = creater2.Create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesTest() =
    let data = AddTwoWaves 0.5 (SoundData(amplitude0 = 0.).Create Sin) (SoundData(amplitude0 = 1.).Create Sin)
    let mockData = (SoundData(amplitude0 = 0.5).Create Sin)

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesWithASilentTest() =
    let data = AddTwoWaves 0.5 (SoundData(amplitude0 = 1.).Create Sin) (SoundData(amplitude0 = 0.).Create Sin)
    let mockData = (SoundData(amplitude0 = 0.5).Create Sin)

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesFirstRatioTest() =
    let data = AddTwoWaves 1. (SoundData(amplitude0 = 1.).Create Sin) (SoundData(amplitude0 = 0.5).Create Sin)
    let mockData = SoundData(amplitude0 = 1.).Create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let AddTwoWavesSecondRatioTest() =
    let data = AddTwoWaves 0 (SoundData(amplitude0 = 1.).Create Sin) (SoundData(amplitude0 = 0.5).Create Sin)
    let mockData = SoundData(amplitude0 = 0.5).Create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())