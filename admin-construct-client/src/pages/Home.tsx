import { Link } from 'react-router-dom';
import { Package, Truck } from 'lucide-react';

export default function Home() {
  return (
    <div className="container mx-auto px-4 py-12">
      <div className="text-center mb-12">
        <h1 className="text-4xl font-bold text-primary mb-4">
          Bienvenido a AdminConstruct
        </h1>
        <p className="text-xl text-gray-600">
          Tu plataforma para adquirir materiales de construcción y alquilar maquinaria
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
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Productos</h2>
            <p className="text-gray-600">
              Explora nuestro catálogo de materiales de construcción
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
            <h2 className="text-2xl font-bold text-gray-800 mb-2">Maquinaria</h2>
            <p className="text-gray-600">
              Alquila maquinaria amarilla para tus proyectos
            </p>
          </div>
        </Link>
      </div>
    </div>
  );
}
