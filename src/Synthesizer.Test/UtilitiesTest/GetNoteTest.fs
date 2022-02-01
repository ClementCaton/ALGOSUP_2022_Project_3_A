module Synthesizer.getNoteTest

open System
open NUnit.Framework

[<SetUp>]
let Setup () =
    ()

[<Test>]
let GetNoteSimple () =
    let Val1 = Math.Round( CalcNoteFreq(Note.A, 4).Output, 2)
    let Val2 = Math.Round( CalcNoteFreq(Note.B, 5).Output, 2)

    Assert.That(Val1, Is.EqualTo(440.))
    Assert.That(Val2, Is.EqualTo(987.77))

[<Test>]
let GetNoteChangeDefaultFreq () =
    let Val1 = Math.Round( CalcNoteFreq(Note.A, 4, 436).Output, 2)
    let Val2 = Math.Round( CalcNoteFreq(Note.B, 5, 444).Output, 2)

    Assert.That(Val1, Is.EqualTo(436.))
    Assert.That(Val2, Is.EqualTo(996.75))

