module Synthesizer.apiTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let getNoteApiTest() =
    let val1 = API.getNote 4 "A"
    let val2 = API.getNote 5 "b"

    Assert.That(val1, Is.EqualTo(440.))
    Assert.That(val2, Is.EqualTo(987.77))

[<Test>]
let getNoteWithOffsetApiTest() =
    let val1 = API.getNoteOffset 4 "A" 436
    let val2 = API.getNoteOffset 5 "b" 444

    Assert.That(val1, Is.EqualTo(436.))
    Assert.That(val2, Is.EqualTo(996.75))