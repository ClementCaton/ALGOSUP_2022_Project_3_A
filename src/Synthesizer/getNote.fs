namespace Synthesizer

type CalcNoteFreq(octav:int, note:string, ?a4Freq0:float) =
    //setting default A4 frequency * octav level
    let startingFreq = (defaultArg a4Freq0 440.) * (2. ** (float octav - 4.))

    //changing to the right note
    let calcNote step = System.Math.Round((startingFreq * (1.05946309436 ** step)), 2)

    let output = 
        match note with
        | _ when note = "C"                 -> calcNote -9.
        | _ when note = "C#" || note = "Db" -> calcNote -8.
        | _ when note = "D"                 -> calcNote -7.
        | _ when note = "D#" || note = "Eb" -> calcNote -6.
        | _ when note = "E"                 -> calcNote -5.
        | _ when note = "F"                 -> calcNote -4.
        | _ when note = "F#" || note = "Gb" -> calcNote -3.
        | _ when note = "G"                 -> calcNote -2.
        | _ when note = "G#" || note = "Ab" -> calcNote -1.
        | _ when note = "A"                 -> calcNote 0.
        | _ when note = "A#" || note = "Bb" -> calcNote 1.
        | _ when note = "B"                 -> calcNote 2.

    do printfn $"output freq= {output}" 

    member x.Output = output