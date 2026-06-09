namespace Jobworld.CarSystems
{
    /// <summary>
    /// 자동차 입력 데이터를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct CarInputData
    {
        public float AccelerateInput;
        public float BrakeInput;
        public float SteeringInput;
    }
}