module fourWaves
    open System
    let pi = Math.PI
    let sinWave frequence amplitude verticalShift phaseShift t  =
        amplitude * sin (2. * pi * t * frequence - phaseShift) + verticalShift

    let sawWave frequence amplitude verticalShift phaseShift t =
        2. * amplitude * (t * frequence - phaseShift - floor (0.5 +  t * frequence - phaseShift)) + verticalShift

    let squareWave frequence amplitude verticalShift phaseShift t =
        amplitude * float (sign (sin (2. * pi * t * frequence - phaseShift))) + verticalShift

    let triangleWave frequence amplitude verticalShift phaseShift t =
        2. * amplitude * asin (sin (2. * pi * t * frequence - phaseShift)) / pi + verticalShift
