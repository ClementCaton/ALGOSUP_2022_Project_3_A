module Synthesizer.utilityTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

let one = Seconds 1.

[<Test>]
let cutStartUtility() =
    let data = API.note one Note.C 4 |> Utility.cutStart 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let cutEndUtility() =
    let data = API.note one Note.C 4 |> Utility.cutEnd 44100. 0.2

    Assert.That(data.Length, Is.EqualTo(44100.*0.8))
    Assert.That(data |> List.max, Is.LessThan(1))
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let cutCornersUtility() =
    let data = API.note one Note.C 4 |> Utility.cutCorners 800 |> Utility.cutEnd 44100. 0.98
    let mockData = API.note one Note.C 4 |> Utility.cutEnd 44100. 0.98

    Assert.That (data, Is.LessThan mockData)
    Assert.That (data, Is.InstanceOf(typeof<List<float>>))

[<Test>]
let addUtility() =
    let data = API.note one Note.C 4
    let data2 = API.note one Note.C 7

    let finalData = Utility.add [data; data2]

    Assert.That(data.Length, Is.EqualTo(44100))
    Assert.That(finalData |> List.max, Is.LessThan(1))
    Assert.That (finalData, Is.InstanceOf(typeof<List<float>>))