module Synthesizer.getNoteTest

open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let getNoteSimple () =
    let val1 = CalcNoteFreq(4, "A").Output
    let val2 = CalcNoteFreq(5, "b").Output

    Assert.That(val1, Is.EqualTo(440.))
    Assert.That(val2, Is.EqualTo(987.77))

[<Test>]
let getNoteChangeDefaultFreq () =
    let val1 = CalcNoteFreq(4, "A", 436).Output
    let val2 = CalcNoteFreq(5, "b", 444).Output

    Assert.That(val1, Is.EqualTo(436.))
    Assert.That(val2, Is.EqualTo(996.75))

