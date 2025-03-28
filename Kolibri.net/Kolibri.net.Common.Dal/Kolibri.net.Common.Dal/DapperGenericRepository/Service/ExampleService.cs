using DapperGenericRepository.Model;
using DapperGenericRepository.Repository;

namespace DapperGenericRepository.Service
{
    public class ExampleService
    {
        public bool Add(Example example)
        {
            bool isAdded = false;
            try
            {
                ExampleRepository exampletRepository = new ExampleRepository();
                isAdded = exampletRepository.Insert(example);
            }
            catch (Exception ex)
            {
            }
            return isAdded;
        }

        public List<Example> GetAll()
        {
            List<Example> examples = new List<Example>();
            try
            {
                ExampleRepository exampletRepository = new ExampleRepository();
                examples = exampletRepository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return examples;
        }

        public Example Get(int Id)
        {
            Example example = new Example();
            try
            {
                ExampleRepository exampletRepository = new ExampleRepository();
                example = exampletRepository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return example;
        }

        public bool Update(Example example)
        {
            bool isUpdated = false;
            try
            {
                ExampleRepository exampletRepository = new ExampleRepository();
                isUpdated = exampletRepository.Update(example);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Example example)
        {
            bool isDeleted = false;
            try
            {
                ExampleRepository exampletRepository = new ExampleRepository();
                isDeleted = exampletRepository.Delete(example);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
    }
}