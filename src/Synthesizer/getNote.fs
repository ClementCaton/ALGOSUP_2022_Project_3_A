namespace Synthesizer

type Note =
    | C = -9
    | Cs = -8 | Db = -8
    | D = -7
    | Ds = -6 | Eb = -6
    | E = -5
    | F = -4
    | Fs = -3 | Gb = -3
    | G = -2
    | Gs = -1 | Ab = -1
    | A = 0
    | As = 1 | Bb = 1
    | B = 2

type CalcNoteFreq(note:Note, octav:int, ?a4Freq0:float) =
    //setting default A4 frequency * octav level
    let startingFreq = (defaultArg a4Freq0 440.) * (2. ** (float octav - 4.))

    //changing to the right note
    let calcNote step = startingFreq * (1.05946309436 ** step)

    member x.Output = 
        System.Math.Round(calcNote (float note), 2)


