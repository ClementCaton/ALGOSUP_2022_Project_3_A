module Synthesizer.SoundDataTest

open System
open System.Collections
open NUnit.Framework
open NUnit.Framework.Constraints

[<SetUp>]
let Setup () =
    ()

[<Test>]
let CreateTest() =
    let SoundDataCreater = new SoundData()
    let Data = SoundDataCreater.Create Silence

    let MockData = List.init (int(44100.*60./90.)) (fun i -> 0.)

    Assert.That (Data, Is.EqualTo MockData)
    Assert.That (Data, Is.InstanceOf(typeof<List<float>>))
