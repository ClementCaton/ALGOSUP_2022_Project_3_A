module Synthesizer.filtersTest

open System
open NUnit.Framework
open Synthesizer.Filter

[<SetUp>]
let Setup () =
    ()

[<Test>]
let changeAmplitudeTest() =
    let creater1 = new createSoundData (amplitude0 = 0.5)
    let data = changeAmplitude 2. (creater1.create Sin)
    let creater2 = new createSoundData (amplitude0 = 1.)
    let mockData = creater2.create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

// [<Test>]
let addTwoWavesTest() =
    let creater0 = new createSoundData (amplitude0 = 0.)
    let creater1 = new createSoundData (amplitude0 = 1.)
    let data = addTwoWaves 0.5 (creater1.create Sin) (creater0.create Sin)
   
    let creater2 = new createSoundData (amplitude0 = 1)
    let mockData = (creater2.create Sin)

    Assert.That(mockData.[200],Is.EqualTo(data.[200]))
    Assert.That(data,Is.InstanceOf<List<float>>())

