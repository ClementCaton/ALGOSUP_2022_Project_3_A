module playSound
    open System.IO
    open SFML.Audio
    open SFML.System

    let playWithOffset stream offset =
        let soundBuffer = new SoundBuffer(stream:MemoryStream)
        let sound = new Sound(soundBuffer)
        let timeOffset = Time.FromSeconds(offset)
        sound.PlayingOffset <- timeOffset
        sound.Play()
        ignore (System.Console.ReadLine())

    let play stream =
        playWithOffset stream (float32(0.))

    