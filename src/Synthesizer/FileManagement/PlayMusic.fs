module playMusic
    open System.IO
    open SFML.Audio
    open SFML.System

    let playWithOffset offset (stream:Stream) =     //if the offset is > to the length of the music it will start from the beginning
        let music = new Music(stream)
        let timeOffset = Time.FromSeconds(offset)
        music.PlayingOffset <- timeOffset
        music.Play()
        ignore (System.Console.ReadLine())

    let play stream =
        playWithOffset (float32(0.)) stream

    let playWithOffsetFromPath offset (filePath:string) =
        let stream = File.Open (filePath, FileMode.Open)
        playWithOffset offset stream
