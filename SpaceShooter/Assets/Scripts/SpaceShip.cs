public class SpaceShip
{
    private int health;
    private float speed;
    private float fireRate;
    public int Health { get => health; set => health = value; }
    public float Speed { get => speed; set => speed = value; }
    public float FireRate { get => fireRate; set => fireRate = value; }
    public SpaceShip(int health, float speed)
    {
        this.Health = health;
        this.Speed = speed;
    }


}
