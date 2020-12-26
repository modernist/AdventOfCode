using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Select(line =>
                    line.Split(new[] {' ', '<', '>', ',', '=', 'x', 'y', 'z'}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(float.Parse).ToArray())
                .Select(values => new Vector3(values[0], values[1], values[2]))
                .Select(pos => new Moon()
                {
                    Position = pos,
                    Velocity = new Vector3()
                });

            Console.WriteLine(Part1(input));

            Console.WriteLine(Part2(input));

            Console.Read();
        }

        static int Part1(IEnumerable<Moon> moons)
        {
            var state = Step(moons.ToArray()).Skip(999).First();
            return state.Sum(moon => moon.Energy);
        }

        static long Part2(IEnumerable<Moon> moons)
        {
            var cycleStepsPerDimension = new long[3];
            var dimensionSelectors = new Func<Moon, (float, float)>[]
                {
                    m => (m.Position.X, m.Velocity.X),
                    m => (m.Position.Y, m.Velocity.Y),
                    m => (m.Position.Z, m.Velocity.Z)
                };

            for (int i = 0; i < 3; i++)
            {
                var states = new HashSet<((float, float), (float, float), (float, float), (float, float))>();
                foreach (var step in Step(moons.ToArray()))
                {
                    var m0 = dimensionSelectors[i](step[0]);
                    var m1 = dimensionSelectors[i](step[1]);
                    var m2 = dimensionSelectors[i](step[2]);
                    var m3 = dimensionSelectors[i](step[3]);
                    var state = (m0, m1, m2, m3);

                    if (states.Contains(state))
                    {
                        break;
                    }
                    states.Add(state);
                }
                cycleStepsPerDimension[i] = states.Count;
            }

            return Lcm(cycleStepsPerDimension[0], Lcm(cycleStepsPerDimension[1], cycleStepsPerDimension[2]));
        }

        static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);

        static long Lcm(long a, long b) => a / Gcd(a, b) * b;

        static IEnumerable<Moon[]> Step(Moon[] moons)
        {
            while (true)
            {
                foreach (var a in moons)
                {
                    foreach (var b in moons)
                    {
                        if (ReferenceEquals(a, b))
                        {
                            continue;
                        }
                        a.ApplyGravity(b);
                    }
                }

                foreach (var moon in moons)
                {
                    moon.ApplyVelocity();
                }

                yield return moons;
            }
        }
    }

    public class Moon : IEquatable<Moon>
    {
        //Vector3 should be safe for x, y, z up to 2^24

        public Vector3 Position;

        public Vector3 Velocity;

        public int PotentialEnergy
        {
            get
            {
                var abs = Vector3.Abs(Position);
                return (int)(abs.X + abs.Y + abs.Z);
            }
        }

        public int KineticEnergy
        {
            get
            {
                var abs = Vector3.Abs(Velocity);
                return (int) (abs.X + abs.Y + abs.Z);
            }
        }

        public int Energy => PotentialEnergy * KineticEnergy;

        public void ApplyGravity(Moon other)
        {
            Velocity.X += Math.Sign(other.Position.X - Position.X);
            Velocity.Y += Math.Sign(other.Position.Y - Position.Y);
            Velocity.Z += Math.Sign(other.Position.Z - Position.Z);
        }

        public void ApplyVelocity()
        {
            Position += Velocity;
        }

        public bool Equals(Moon other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Position.Equals(other.Position) && Velocity.Equals(other.Velocity);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Moon) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Position.GetHashCode() * 397) ^ Velocity.GetHashCode();
            }
        }
    }
}
