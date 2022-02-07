module PlayMusic
    open System.IO
    open SFML.Audio
    open SFML.System
    open System.Diagnostics



    /// <summary>
    /// Plays a stream with an offset on Windows
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="stream">Stream to play</param>
    
    let PlayWithOffset (offset:float32) (stream:Stream) =     //if the offset is > to the length of the music it will start from the beginning
        let music = new Music(stream)
        let timeOffset = Time.FromSeconds(offset)
        music.PlayingOffset <- timeOffset
        music.Play()
        ignore (System.Console.ReadLine()) // press enter to end it // thread.sleep(1) could be another solution
    


    /// <summary>
    /// Plays a stream with an offset on Mac
    /// </summary>
    /// <param name="file">Path of the file to play</param>
    /// <param name="offset">When to start playing in seconds</param>
    
    let PlayMac (file:string) (offset:float) =
        Process.Start("afplay", file + " -t " + string offset)



    /// <summary>
    /// Plays a stream on Windows
    /// </summary>
    /// <param name="stream">Stream to play</param>
    
    let Play stream =
        PlayWithOffset (float32(0.)) stream



    /// <summary>
    /// Plays a file with an offset on Windows
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="filePath">Path of the file to play</param>
    
    let PlayWithOffsetFromPath (offset:float32) (filePath:string) =
        let stream = File.Open (filePath, FileMode.Open)
        PlayWithOffset offset stream
