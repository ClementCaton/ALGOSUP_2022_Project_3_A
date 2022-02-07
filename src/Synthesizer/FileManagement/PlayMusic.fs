module PlayMusic
    open System.IO
    open SFML.Audio
    open SFML.System
    open System.Diagnostics



    /// <summary>
    /// Play sound from a stream that contains sound data
    /// </summary>
    /// <param name="offset">The second at which the sound starts to play</param>
    /// <param name="stream">The stream containing the data</param>
    
    let PlayWithOffset (offset:float32) (stream:Stream) =     //if the offset is > to the length of the music it will start from the beginning
        let music = new Music(stream)
        let timeOffset = Time.FromSeconds(float32(offset))
        music.PlayingOffset <- timeOffset
        music.Play()
        ignore (System.Console.ReadLine())  //This line allows the sfml to play until the user press the enter key
    


    /// <summary>
    /// Plays a stream with an offset on Mac
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="file">Path of the file to play</param>
    
    let PlayMac (offset:float) (file:string) =
        Process.Start("afplay", file + " -t " + string offset)


    /// <summary>
    /// Plays a stream on Windows
    /// </summary>
    /// <param name="stream">Stream to play</param>
    
    let Play stream =
        PlayWithOffset 0. stream



    /// <summary>
    /// Plays a file with an offset on Windows
    /// </summary>
    /// <param name="offset">When to start playing in seconds</param>
    /// <param name="filePath">Path of the file to play</param>
    
    let PlayWithOffsetFromPath (offset:float32) (filePath:string) =
        let stream = File.Open (filePath, FileMode.Open)
        PlayWithOffset offset stream
