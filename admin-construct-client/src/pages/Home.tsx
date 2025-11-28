import { Link } from 'react-router-dom';
import { Package, Truck } from 'lucide-react';

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-12">
      <div className="text-center mb-12">
        <h1 className="text-4xl font-bold text-primary mb-4">
          Welcome to AdminConstruct
        </h1>
        <p className="text-xl text-gray-600">
          Your platform for purchasing construction materials and renting machinery
        </p>
      </div>

      <div className="grid md:grid-cols-2 gap-8 max-w-4xl mx-auto">
        <Link
          to="/products"
          className="bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition transform hover:-translate-y-1"
        >
          <div className="flex flex-col items-center text-center">
            <div className="bg-primary text-white p-6 rounded-full mb-4">
              <Package size={48} />
            </div>
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Products</h2>
            <p className="text-gray-600">
              Explore our catalog of construction materials
            </p>
          </div>
        </Link>

        <Link
          to="/machinery"
          className="bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition transform hover:-translate-y-1"
        >
          <div className="flex flex-col items-center text-center">
            <div className="bg-secondary text-white p-6 rounded-full mb-4">
              <Truck size={48} />
            </div>
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Machinery</h2>
            <p className="text-gray-600">
              Rent heavy machinery for your projects
            </p>
          </div>
        </Link>
      </div>
    </div>
  );
}
