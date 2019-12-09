public class JunctionXWithPriority : JunctionX
{
    protected override void EvaluateCars()
    {
        cantGo.ForEach(car =>
        {
            if (rightTurns.Contains(car.Item2) &&
                    canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (straights.Contains(car.Item2) && canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });
        cantGo.ForEach(car =>
        {
            if (leftTurns.Contains(car.Item2) && canGo.TrueForAll(otherCar => !Intersect(car.Item2, otherCar.Item2)))
                canGo.Add(car);
        });

        canGo.ForEach(it =>
        {
            it.Item1.forbiddenTraffic = false;
            cantGo.Remove(it);
        });
        cantGo.ForEach(it => it.Item1.forbiddenTraffic = true);
    }
}