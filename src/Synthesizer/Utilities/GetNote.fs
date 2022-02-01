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

type CalcNoteFreq(Note:Note, Octav:int, ?A4Freq0:float) =
    //setting default A4 frequency * octav level
    let StartingFreq = (DefaultArg A4Freq0 440.) * (2. ** (float Octav - 4.))

    //changing to the right note
    let CalcNote Step = StartingFreq * (1.05946309436 ** Step)

    member x.Output = 
        CalcNote (float Note)


