using System.Collections.Generic;

namespace BudgetCalculator
{
    public interface IRepository<T>
    {
        List<Budget> GetAll();
    }
}   