import { useState, useEffect } from 'react';
import { productService, type MachineryDto } from '../services/productService.ts';
import { Calendar } from 'lucide-react';
import { notifications } from '../utils/notifications.ts';
import { useCart } from '../context/CartContext';

export default function Machinery() {
  const [machinery, setMachinery] = useState<MachineryDto[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [selectedMachinery, setSelectedMachinery] = useState<MachineryDto | null>(null);
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const { addToCart } = useCart();


  useEffect(() => {
    loadMachinery();
  }, [page]);

  const loadMachinery = async () => {
    setLoading(true);
    try {
      const result = await productService.getMachinery(page, 9);
      setMachinery(result.items);
      setTotalPages(result.totalPages);
    } catch (error) {
      console.error('Error loading machinery:', error);
      notifications.error('Error loading machinery');
    } finally {
      setLoading(false);
    }
  };

  const handleRent = (item: MachineryDto) => {
    setSelectedMachinery(item);
  };

  const confirmRental = async () => {
    if (!selectedMachinery || !startDate || !endDate) {
      notifications.warning('Please complete all fields');
      return;
    }

    addToCart({
      id: selectedMachinery.id,
      name: selectedMachinery.name,
      price: selectedMachinery.price,
      type: 'rental',
      startDate,
      endDate,
      quantity: 1
    });
    
    notifications.success('Rental added to cart');
    setSelectedMachinery(null);
    setStartDate('');
    setEndDate('');
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-primary mb-8">Machinery Catalog</h1>

      {loading ? (
        <div className="text-center py-12">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {machinery.map((item) => (
              <div key={item.id} className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-xl transition">
                <div className="p-6">
                  <h3 className="text-xl font-semibold text-gray-800 mb-2">{item.name}</h3>
                  {item.description && (
                    <p className="text-gray-600 mb-2 text-sm">{item.description}</p>
                  )}
                  <div className="flex justify-between items-center mb-2">
                    <span className="text-sm text-gray-500">Stock: {item.stock}</span>
                    <span className={`px-2 py-1 rounded text-xs ${item.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                      {item.isActive ? 'Available' : 'Unavailable'}
                    </span>
                  </div>
                  <p className="text-2xl font-bold text-secondary mb-4">
                    ${item.price.toFixed(2)}/day
                  </p>
                  <button
                    onClick={() => handleRent(item)}
                    disabled={!item.isActive || item.stock === 0}
                    className="w-full bg-secondary text-white py-2 rounded-lg hover:bg-secondary-dark transition flex items-center justify-center space-x-2 disabled:opacity-50 disabled:cursor-not-allowed font-medium"
                  >
                    <Calendar size={20} />
                    <span>Rent</span>
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Pagination */}
          <div className="flex justify-center items-center space-x-4 mt-8">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 bg-primary text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-blue-900 transition"
            >
              Previous
            </button>
            <span className="text-gray-700">
              Page {page} of {totalPages}
            </span>
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 bg-primary text-white rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-blue-900 transition"
            >
              Next
            </button>
          </div>
        </>
      )}

      {/* Rental Modal */}
      {selectedMachinery && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full">
            <h3 className="text-2xl font-bold text-primary mb-4">Rent: {selectedMachinery.name}</h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Start Date
                </label>
                <input
                  type="date"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                  min={new Date().toISOString().split('T')[0]}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  End Date
                </label>
                <input
                  type="date"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                  min={startDate || new Date().toISOString().split('T')[0]}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary"
                />
              </div>
              <div className="flex space-x-4">
                <button
                  onClick={confirmRental}
                  className="flex-1 bg-secondary text-white py-2 rounded-lg hover:bg-orange-600 transition"
                >
                  Confirm
                </button>
                <button
                  onClick={() => {
                    setSelectedMachinery(null);
                    setStartDate('');
                    setEndDate('');
                  }}
                  className="flex-1 bg-gray-300 text-gray-700 py-2 rounded-lg hover:bg-gray-400 transition"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
