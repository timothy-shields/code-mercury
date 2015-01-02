using CodeMercury.Domain.Models;
using System;

namespace CodeMercury.Services
{
    public interface IArgumentMaterializer
    {
        object Materialize(Type type, Argument argument);
    }
}
