// Бизнес-правила Courier:

// Courier - это курьер, он состоит из:
using System.ComponentModel.DataAnnotations;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate
{
    public class Courier : Aggregate
    {
        // Name (имя курьера)
        public string Name { get; protected set; }
        // Transport (транспорт курьера)

        public Transport Transport;

        // Location (местоположение курьера)

        public Location Location;

        // Status (статус курьера)
        public CourierStatusEnum CourierStatus;

        public Courier(string name, Transport transport)
        {
            Name = name;
            Transport = transport;
            Location = new Location(1, 1);
            CourierStatus = CourierStatusEnum.NotAvailable;
        }
        // Курьер может начать рабочий день
        // При этом статус меняется на Ready (готов к работе)
        // Если он занят (выполняет заказ), то он не может это сделать
        public void CourierCanStart()
        {
            if (CourierStatus == CourierStatusEnum.Busy) return;
            CourierStatus = CourierStatusEnum.Ready;
        }


        // Курьер может закончить рабочий день
        // При этом статус меняется на NotAvailable (недоступен)
        // Если он занят (выполняет заказ), то он не может это сделать

        public void CourierStop()
        {
            if (CourierStatus == CourierStatusEnum.Busy) return;
            CourierStatus = CourierStatusEnum.NotAvailable;
        }
        // Заказ может быть назначен на курьера
        // При этом статус курьера меняется на Busy (занят)
        // Если курьер уже Busy (занят), то нельзя его занять, это ошибка
        // Если курьер NotAvailable (недоступен), то нельзя его занять, это ошибка

        public void CourierStart()
        {
            if (CourierStatus == CourierStatusEnum.Busy ||
                CourierStatus == CourierStatusEnum.NotAvailable)
                throw new Exception("Курьер недоступен");
            CourierStatus = CourierStatusEnum.Busy;
        }

        public void Move(Location targetLocation)
        {
            var step = Transport.Speed;
            var distance = targetLocation.GetDistanceTo(Location);
            if (distance <= 0) return;

            while (true)
            {
                var diffX = targetLocation.X - Location.X;
                var diffY = targetLocation.Y - Location.Y;

                if (diffX <= 0 && diffY <= 0) { break; }
                if (step <= diffX)
                    Location = new Location(Location.X + step,Location.Y);

                if (step <= diffY)
                    Location = new Location(Location.X,Location.Y+step);

            }
        }
    
    // Курьер может переместиться на один шаг в сторону Location заказа
    // Размер шага курьера равен скорости его транспорта, к примеру, если скорость велосипеда = 2, 
    // это значит, что шаг курьера на велосипеде = 2
    // Курьер может ходить как горизонтально, так и вертикально, но не наискосок. 
    // К примеру, если шаг курьера =2, то он может сделать 1 шаг по горизонтали и 1 шаг 
    // по вертикали или по 2 шага по прямой.
    // Если транспорт курьера движется, к примеру, со скоростью 4 клетки за 1 шаг, 
    // а заказ находится ближе, к примеру в 2 клетках, то курьер должен переместиться 
    // только до Location заказа
    // Если курьер достиг Location заказа, то
    // Заказ завершается (переходит в статус Completed)
    // Курьер становится свободным (переходит в статус Ready)
    // Курьер должен уметь возвращать количество шагов, которое он потенциально 
    // затратит на путь до локации заказа. При расчете нужно участь скорость 
    // транспорта курьера. К примеру:
    // Есть курьер на велосипеде
    // Курьер находится в точке (1,1)
    // Заказ находится в точке (5,5)
    // Курьеру надо пройти 4 клетки по горизонтали и 4 по вертикали, чтобы оказаться в точке доставки заказа. Суммарная дистанция равна - 8 клеток
    // Транспорт у курьера "Велосипед" и он едет со скоростью 2 клетки за 1 шаг
    // Итого - курьеру нужно 4 шага (по 2 клетки), чтобы доставить заказ
}
}