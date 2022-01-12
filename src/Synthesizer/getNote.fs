namespace Synthesizer

type CalcNoteFreq(octav:int, note:string, ?a4Freq0:float) =
    //setting default A4 frequency * octav level
    let startingFreq = (defaultArg a4Freq0 440.) * (2. ** (float octav - 4.))

    //changing to the right note
    let calcNote step = System.Math.Round((startingFreq * (1.05946309436 ** step)), 2)

    member x.Output = 
        match note with
        | _ when note.ToUpper() = "C"                 -> calcNote -9.
        | _ when note.ToUpper() = "C#" || note = "DB" -> calcNote -8.
        | _ when note.ToUpper() = "D"                 -> calcNote -7.
        | _ when note.ToUpper() = "D#" || note = "EB" -> calcNote -6.
        | _ when note.ToUpper() = "E"                 -> calcNote -5.
        | _ when note.ToUpper() = "F"                 -> calcNote -4.
        | _ when note.ToUpper() = "F#" || note = "GB" -> calcNote -3.
        | _ when note.ToUpper() = "G"                 -> calcNote -2.
        | _ when note.ToUpper() = "G#" || note = "AB" -> calcNote -1.
        | _ when note.ToUpper() = "A"                 -> calcNote 0.
        | _ when note.ToUpper() = "A#" || note = "BB" -> calcNote 1.
        | _ when note.ToUpper() = "B"                 -> calcNote 2.
        | _ -> -1.