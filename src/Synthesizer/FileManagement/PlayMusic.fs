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
    
    let PlayWithOffset offset (stream:Stream) =     //if the offset is > to the length of the music it will start from the beginning
        let music = new Music(stream)
        let timeOffset = Time.FromSeconds(float32(offset))
        music.PlayingOffset <- timeOffset
        music.Play()
        ignore (System.Console.ReadLine())  //This line allows the sfml to play until the user press the enter key
    
    let PlayMac (file:string) offset =
        Process.Start("afplay", file + " -t " + string offset)


    /// <summary>
    /// This function is just an alias for PlayWithOffset that gives an offset of 0 seconds. It requires only one parameter
    /// </summary>
    /// <param name="stream">The stream that contains the data</param>
    
    let Play stream =
        PlayWithOffset 0. stream



    /// <summary>
    /// Open a file from the path given and play the sound.
    /// </summary>
    /// <param name="offset">The second at which the sound starts to play</param>
    /// <param name="filePath">The path where the file is.</param>
    
    let PlayWithOffsetFromPath offset (filePath:string) =
        let stream = File.Open (filePath, FileMode.Open)
        PlayWithOffset offset stream
