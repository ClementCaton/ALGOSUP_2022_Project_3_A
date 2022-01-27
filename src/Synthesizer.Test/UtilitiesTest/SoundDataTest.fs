module Synthesizer.SoundDataTest

open System
open System.Collections
open NUnit.Framework
open NUnit.Framework.Constraints

[<SetUp>]
let Setup () =
    ()

[<Test>]
let createTest() =
    let soundDataCreater = new SoundData()
    let data = soundDataCreater.create Silence

    let mockData = List.init (int(44100.*60./90.)) (fun i -> 0.)

    Assert.That (data, Is.EqualTo mockData)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))
