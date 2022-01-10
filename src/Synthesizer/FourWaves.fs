module fourWaves
    open System
    let pi = Math.PI
    let sinWave frequence amplitude t  =
        amplitude * sin (2. * pi * t * frequence)

    let sawWave frequence amplitude  t =
        2. * amplitude * (t * frequence - floor (0.5 +  t * frequence))

    let squareWave frequence amplitude t =
        amplitude * float (sign (sin (2. * pi * t * frequence)))

    let triangleWave frequence amplitude t =
        2. * amplitude * asin (sin (2. * pi * t * frequence)) / pi
