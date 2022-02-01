module FourWaves
    open System
    let Pi = Math.PI
    
    let SinWave Frequence Amplitude VerticalShift PhaseShift tT  =
        Amplitude * Sin (2. * Pi * T * Frequence - PhaseShift) + VerticalShift

    let SawWave Frequence Amplitude VerticalShift PhaseShift T =
        2. * Amplitude * (T * Frequence - PhaseShift - floor (0.5 +  T * Frequence - PhaseShift)) + VerticalShift

    let SquareWave Frequence amplitude VerticalShift PhaseShift T =
        Amplitude * float (sign (sin (2. * Pi * T * Frequence - PhaseShift))) + VerticalShift

    let TriangleWave Frequence amplitude VerticalShift PhaseShift T =
        2. * Amplitude * Asin (sin (2. * Pi * T * Frequence - PhaseShift)) / Pi + VerticalShift
