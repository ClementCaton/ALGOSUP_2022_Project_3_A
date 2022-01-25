module Synthesizer.filtersTest

open System
open NUnit.Framework
open Synthesizer.Filter

[<SetUp>]
let Setup () =
    ()

[<Test>]
let changeAmplitudeTest() =
    let duration = Quarter // In seconds
    let sampleRate = 44100.
    let arraySize = 44100. 
    let bpm = 90.
    let phaseShift = 0.
    let verticalShift =  0.
    let frequency =  440.
    let overDrive =  1.

    let creater1 = new createSoundData (overDrive, duration, arraySize, 0.5, verticalShift, phaseShift, frequency, sampleRate, bpm)
    let data = changeAmplitude 2. (creater1.create Sin)
    let creater2 = new createSoundData (overDrive, duration, arraySize, 1., verticalShift, phaseShift, frequency, sampleRate, bpm)
    let mockData = creater2.create Sin

    Assert.That(mockData,Is.EqualTo(data))
    Assert.That(data,Is.InstanceOf<List<float>>())

[<Test>]
let addTwoWavesTest() =
    let duration = Quarter // In seconds
    let sampleRate = 44100.
    let arraySize = 44100. 
    let bpm = 90.
    let phaseShift = 0.
    let verticalShift =  0.
    let frequency =  440.
    let overDrive =  1.

    let creater0 = new createSoundData (overDrive, duration, arraySize, 0, verticalShift, phaseShift, frequency, sampleRate, bpm)
    let creater1 = new createSoundData (overDrive, duration, arraySize, 1, verticalShift, phaseShift, frequency, sampleRate, bpm)
    let data = addTwoWaves 0.5 (creater1.create Sin) (creater0.create Sin)
   
    let creater2 = new createSoundData (overDrive, duration, arraySize, 1, verticalShift, phaseShift, frequency, sampleRate, bpm)
    let mockData = (creater2.create Sin)

    Assert.That(mockData.[200],Is.EqualTo(data.[200]))
    Assert.That(data,Is.InstanceOf<List<float>>())

