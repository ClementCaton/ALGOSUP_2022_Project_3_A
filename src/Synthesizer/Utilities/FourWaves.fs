module FourWaves
    open System
    let Pi = Math.PI
    
    let SinWave frequence amplitude verticalShift phaseShift t  =
        amplitude * sin (2. * Pi * t * frequence - phaseShift) + verticalShift

    let SawWave frequence amplitude verticalShift phaseShift t =
        2. * amplitude * (t * frequence - phaseShift - floor (0.5 +  t * frequence - phaseShift)) + verticalShift

    let SquareWave frequence amplitude verticalShift phaseShift t =
        amplitude * float (sign (sin (2. * Pi * t * frequence - phaseShift))) + verticalShift

    let TriangleWave frequence amplitude verticalShift phaseShift t =
        2. * amplitude * asin (sin (2. * Pi * t * frequence - phaseShift)) / Pi + verticalShift
