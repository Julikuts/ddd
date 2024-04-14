using CSharpFunctionalExtensions;

namespace DeliveryApp.Core.Domain.SharedKernel
{
    // Бизнес-правила Location:

    // Location - это координата на доске, она состоит из X(горизонталь) и Y (вертикаль)
    // Должна быть возможность рассчитать расстояние между двумя  Location
    // Расстояние между Location - это количество ходов по X и Y, которое необходимо сделать курьеру чтобы достигнуть точки. (шаги по X + шаги по Y)
    // Минимально возможная для установки координата 1,1
    // Максимально возможная для установки координата 10,10
    // 2 координаты равны, если их X и Y равны, обеспечьте функционал проверки на эквивалентность
    // Нельзя изменять объект Location после создания 
    public class Location : ValueObject
    {
        /// <summary>
        ///  Конструктор создания класса с координатами
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <exception cref="Exception"></exception>
        public Location(int x, int y)
        {
            if (x < 1 || x > 10 || y < 1 || y > 10)
                throw new Exception("Значение должно быть от 1 до 10");

            X = x;
            Y = y;

        }

        /// <summary>
        /// Координата Х
        /// </summary>
        public int X { get; protected set; }

        /// <summary>
        /// Координата Y
        /// </summary>
        public int Y { get; protected set; }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return X;
            yield return Y;
        }

        /// <summary>
        ///  расчитывает расстояние по координатам
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public int GetDistanceTo(Location to) => Math.Abs(to.X - X) + Math.Abs(to.Y - Y);
    }
}