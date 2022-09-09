namespace mca_light_extension_core.Domain
{
    public enum McaExitCode
    {
        RC_No_Error                = 0,
        RC_OverDate                = 10,   //Programma is over de houdbaarheidsdatum
        RC_No_RunArgumenten        = 20,   //Er zijn geen argumenten opgegeven voor het programma
        RC_No_RootDirectory        = 30,   //Root voor een output-directory bestaat niet
        RC_No_OUTPUT_RootDirectory = 33,   //Root output-directory bestaat niet
        RC_No_INPUT_RootDirectory  = 35,   //Root MCA-file bestaat niet
        RC_No_MCAbestand           = 40,   //MCA-input-bestand bestaat niet
        RC_Overflow_name_table     = 43,   //Teveel items; MCA-input-bestand is te groot
        RC_DataFout_MCAbestand     = 45,   //MCA-input-bestand heeft parser fouten
        RC_NO_GridOpdrachtBestand  = 50,   //GRID-opdracht-bestand bestaat niet
        RC_DataFout_GRIDopdracht   = 55,   //GRID-opdracht-bestand heeft parser fouten
        RC_NO_ETMbestand           = 60,   //ETM-bestand bestaat niet
        RC_DataFout_ETMbestand     = 65,   //ETM-bestand heeft parser fouten
        AWS_Wrapper_Error          = 99    //Error in application starting MCA light
    }
}
