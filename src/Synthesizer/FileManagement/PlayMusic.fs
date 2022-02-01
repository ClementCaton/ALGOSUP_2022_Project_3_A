module PlayMusic
    open System.IO
    open SFML.Audio
    open SFML.System

    let PlayWithOffset Offset (stream:Stream) =     //if the offset is > to the length of the music it will start from the beginning
        let Music = new Music(Stream)
        let TimeOffset = Time.FromSeconds(Offset)
        music.PlayingOffset <- TimeOffset
        music.Play()
        ignore (System.Console.ReadLine())

    let Play Stream =
        PlayWithOffset (float32(0.)) Stream

    let PlayWithOffsetFromPath Offset (FilePath:string) =
        let Stream = File.Open (FilePath, FileMode.Open)
        PlayWithOffset Offset Stream
