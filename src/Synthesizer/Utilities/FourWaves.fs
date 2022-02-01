module FourWaves
    open System
    let pi = Math.PI
    
    let SinWave frequency amplitude verticalShift phaseShift t  =
        amplitude * sin (2. * pi * t * frequency - phaseShift) + verticalShift

    let SawWave frequency amplitude verticalShift phaseShift t =
        2. * amplitude * (t * frequency - phaseShift - floor (0.5 +  t * frequency - phaseShift)) + verticalShift

    let SquareWave frequency amplitude verticalShift phaseShift t =
        amplitude * float (sign (sin (2. * pi * t * frequency - phaseShift))) + verticalShift

    let TriangleWave frequency amplitude verticalShift phaseShift t =
        2. * amplitude * asin (sin (2. * pi * t * frequency - phaseShift)) / pi + verticalShift
